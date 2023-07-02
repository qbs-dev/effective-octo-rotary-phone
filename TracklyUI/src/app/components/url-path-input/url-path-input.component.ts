import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { UrlService } from 'src/app/services/url.service';
import { Url } from 'src/app/models/url.model';
import { HttpErrorResponse } from '@angular/common/http';
import {
  createUrlPathValidator,
  getErrorMessage,
} from 'src/app/helpers/custom-validators';

@Component({
  selector: 'app-url-path-input',
  templateUrl: './url-path-input.component.html',
  styleUrls: ['./url-path-input.component.less'],
})
export class UrlPathInputComponent {
  @Output() UrlPathSelectEvent = new EventEmitter<string | null>();
  getErrorMessage = getErrorMessage;
  constructor(
    private urlService: UrlService,
    private errorService: HttpErrorResponseService,
    private snack: SnackNotifyComponent
  ) {
    this.inputForm = new FormGroup({
      urlPathCtrl: this.urlPathCtrl,
    });
  }

  inputForm: FormGroup;
  urlPathCtrl = new FormControl<string>('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(64),
    createUrlPathValidator(),
  ]);
  isChecking = false;

  checkIfUrlPathIsAvailable() {
    this.UrlPathSelectEvent.emit(null);
    this.isChecking = true;
    console.log(`checking urlPath: ${this.urlPathCtrl.value!}`);
    this.urlService.findUrlByPath(this.urlPathCtrl.value!).subscribe({
      next: (res: Url) => {
        this.UrlPathSelectEvent.emit(null);
        this.isChecking = false;
        this.snack.openSnackBar('url with given path already exists', 'OK');
      },
      error: (error: HttpErrorResponse) => {
        console.log(`error status: ${error.status}`);
        if (error.status === 404) {
          this.UrlPathSelectEvent.emit(this.urlPathCtrl.value!);
          this.snack.openSnackBar(
            `Url path set as \"${this.urlPathCtrl.value!}\"`,
            'OK'
          );
        } else {
          this.snack.openSnackBar(
            `Error: ` + this.errorService.getStringFromError(error),
            'OK'
          );
        }
        this.isChecking = false;
      },
    });
  }
}
