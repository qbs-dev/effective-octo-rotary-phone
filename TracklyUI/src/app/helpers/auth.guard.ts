import { CanActivateFn, Router } from "@angular/router";
import { AuthService } from "../services/auth.service";
import { inject } from "@angular/core";

export function hasLoggedInAuthGuard(): CanActivateFn
{
  return () =>
  {
    const authService = inject(AuthService);
    const router = inject(Router);

    return authService.isLoggedIn() || router.createUrlTree(['/forbidden']);
  };
}
