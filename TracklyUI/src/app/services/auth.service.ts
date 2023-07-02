import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { sha3_256 } from '@noble/hashes/sha3';
import FingerprintJS from "@fingerprintjs/fingerprintjs";
import { bytesToHex } from '@noble/hashes/utils';
import { AuthRequest, AuthResponse } from 'src/app/models/auth.model';
import { MessageResponse } from '../models/common.model';

const endpoint = 'api/auth'
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

@Injectable({
  providedIn: 'root'
})
export class AuthService
{
  private fp = FingerprintJS.load();
  constructor(private http: HttpClient) { }

  fingerprint = this.getFingerprint().then(
    resFingerprint => { return resFingerprint },
    error => { return null }
  );

  login(authReq: AuthRequest): Observable<AuthResponse>
  {
    authReq.password = bytesToHex(sha3_256(authReq.password));
    return this.http.post<AuthResponse>(endpoint + '/login', authReq, httpOptions);
  }

  logout(): Observable<MessageResponse>
  {
    return this.http.post<MessageResponse>(endpoint + '/logout', httpOptions);
  }

  refresh(fingerprint: string): Observable<AuthResponse>
  {
    return this.http.post<AuthResponse>(endpoint + `/refresh?fingerprint=${fingerprint}`, httpOptions);
  }

  parseJwt(token: string)
  {
    try
    {
      return JSON.parse(window.atob(token.split('.')[1]));
    } catch (e)
    {
      return null;
    }
  }

  saveData(access: string)
  {
    localStorage.setItem('access', access);
    const jwtPayload = this.parseJwt(access);
    localStorage.setItem('userId', jwtPayload.userId);
  }

  isLoggedIn()
  {
    return localStorage.getItem('userId') !== null;
  }

  getUserId(): number
  {
    return Number(localStorage.getItem('userId'));
  }

  async getFingerprint(): Promise<string>
  {
    const fingerprint = localStorage.getItem('fingerprint');
    if (fingerprint === null)
    {
      const { visitorId } = await (await this.fp).get();
      localStorage.setItem('fingerprint', visitorId);
      return visitorId;
    } else
    {
      return fingerprint;
    }
  }

  forceLogoutCleanup()
  {
    localStorage.removeItem('userId');
    localStorage.removeItem('access');
  }
}

