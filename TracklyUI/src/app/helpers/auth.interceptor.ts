import { Injectable } from '@angular/core';
import
{
  HTTP_INTERCEPTORS,
  HttpRequest,
  HttpHandler,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthResponse } from '../models/auth.model';

const TOKEN_HEADER_KEY = 'Authorization';
@Injectable()
export class AuthInterceptor implements HttpInterceptor
{
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any>
  {
    let authReq = req;
    const token = localStorage.getItem("access");
    if (token != null)
    {
      authReq = this.addTokenHeader(req, token);
    }
    return next.handle(authReq).pipe(catchError(error =>
    {
      if ((error instanceof HttpErrorResponse)
        && !authReq.url.includes('auth/login')
        && error.status === 401)
      {
        return this.handle401Error(authReq, next);
      }
      return throwError(error);
    }));
  }
  private handle401Error(request: HttpRequest<any>, next: HttpHandler)
  {
    if (!this.isRefreshing)
    {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);
      const fingerprint = localStorage.getItem("fingerprint");
      if (fingerprint)
        return this.authService.refresh(fingerprint).pipe(
          switchMap((result: AuthResponse) =>
          {
            this.isRefreshing = false;
            this.authService.saveData(result.accessToken);
            this.refreshTokenSubject.next(result.accessToken);
            return next.handle(this.addTokenHeader(request, result.accessToken));
          }),
          catchError((err) =>
          {
            this.isRefreshing = false;
            this.authService.logout();
            this.authService.forceLogoutCleanup();
            return throwError(err);
          })
        );
    }
    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token)))
    );
  }
  private addTokenHeader(request: HttpRequest<any>, token: string)
  {
    return request.clone({
      headers: request.headers.set(TOKEN_HEADER_KEY, `Bearer ${token}`)
    });
  }
}

export const authInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
];
