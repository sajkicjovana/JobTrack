import { inject } from '@angular/core';
import {
  CanActivateFn,
  Router
} from '@angular/router';

import { Auth } from '../services/auth';

export const userGuard: CanActivateFn = () => {
  const authService = inject(Auth);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  if (!authService.isAdmin()) {
    return true;
  }

  return router.createUrlTree(['/admin']);
};