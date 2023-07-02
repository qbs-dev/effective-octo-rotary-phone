import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ArdalisResultError } from '../models/common.model';

@Injectable({
  providedIn: 'root'
})
export class HttpErrorResponseService {

  constructor() { }

  getStringFromError(error: HttpErrorResponse) {
    switch (error.status) {
      case 422:
        const ardalisResultError = error.error as ArdalisResultError;
        return ardalisResultError.detail.split("* ", 2)[1];
      case 403:
        return 'forbidden';
      case 504:
        return 'gateway timeout';
      default:
        return 'unknown server error';
    }
  }
}
