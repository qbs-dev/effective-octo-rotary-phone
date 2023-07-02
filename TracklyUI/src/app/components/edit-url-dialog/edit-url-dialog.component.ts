import { Component, Inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import {
  createUrlPathValidator,
  createUrlValidator,
  getErrorMessage,
} from 'src/app/helpers/custom-validators';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { UrlService } from 'src/app/services/url.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { Url, UrlEditRequest } from 'src/app/models/url.model';
import { MessageResponse } from 'src/app/models/common.model';

@Component({
  selector: 'app-edit-url-dialog',
  templateUrl: './edit-url-dialog.component.html',
  styleUrls: ['./edit-url-dialog.component.less'],
})
export class EditUrlDialogComponent {
  isLoading: boolean = true;
  isAdding: boolean = false;

  editUrlForm: FormGroup = null!;
  getErrorMessage = getErrorMessage;

  id: number = 0;
  isDeleteConfirmed = new FormControl<boolean>(false);

  newPath = new FormControl('', [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(64),
    createUrlPathValidator(),
  ]);
  targetUrl = new FormControl('', [
    Validators.required,
    Validators.minLength(6),
    Validators.maxLength(128),
    createUrlValidator(),
  ]);
  isActive = new FormControl<boolean>(false, [Validators.required]);
  description = new FormControl('', [Validators.maxLength(256)]);

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private formBuilder: FormBuilder,
    private router: Router,
    private errorService: HttpErrorResponseService,
    private urlService: UrlService,
    private snack: SnackNotifyComponent,
    private dialogRef: MatDialogRef<EditUrlDialogComponent>
  ) {
    this.buildForm();
    this.isLoading = false;
  }

  buildForm(): void {
    this.editUrlForm = this.formBuilder.group({
      isDeleteConfirmed: this.isDeleteConfirmed,
      newPath: this.newPath,
      targetUrl: this.targetUrl,
      isActive: this.isActive,
      description: this.description,
    });

    if (this.data.isAdding) {
      this.isAdding = true;
    } else {
      if (this.data.originalUrl) {
        const url = this.data.originalUrl as Url;
        this.id = url.id;
        this.editUrlForm.setValue({
          newPath: url.newPath,
          targetUrl: url.targetUrl,
          isActive: url.isActive,
          description: url.description,
        });
      }
    }
  }

  sendForm(): void {
    const urlEditRequest: UrlEditRequest = {
      id: this.id,
      newPath: this.newPath.value!,
      targetUrl: this.targetUrl.value!,
      isActive: this.isActive.value!,
      description: this.description.value!,
    };

    this.isLoading = true;
    if (this.isAdding) {
      this.urlService.createUrl(urlEditRequest).subscribe({
        next: (res: Url) => {
          this.snack.openSnackBar('New Url was added', 'OK');
          this.isLoading = false;
          this.dialogRef.close();
          this.router.navigateByUrl(`/urls/${res.id}`);
        },
        error: (error) => {
          this.snack.openSnackBar(
            'Error: ' + this.errorService.getStringFromError(error),
            'OK'
          );
          this.isLoading = false;
        },
      });
    } else {
      this.urlService.editUrlDetails(urlEditRequest).subscribe({
        next: (res: Url) => {
          this.snack.openSnackBar('Url details were changed', 'OK');
          this.isLoading = false;
          this.dialogRef.close();
          this.forceReload();
        },
        error: (error) => {
          this.snack.openSnackBar(
            'Error: ' + this.errorService.getStringFromError(error),
            'OK'
          );
          this.isLoading = false;
        },
      });
    }
  }

  deleteUrl() {
    this.isLoading = true;
    this.urlService.deleteUrl(this.id).subscribe({
      next: (res: MessageResponse) => {
        this.snack.openSnackBar(res.message, 'OK');
        this.isLoading = false;
        this.dialogRef.close();
        this.router.navigateByUrl(`/urls`);
      },
      error: (error) => {
        this.snack.openSnackBar(
          'Error: ' + this.errorService.getStringFromError(error),
          'OK'
        );
        this.isLoading = false;
      },
    });
  }

  setUrlPath($event: string | null) {
    this.newPath.setValue($event);
  }

  forceReload() {
    let currentUrl = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
      console.log(currentUrl);
    });
  }
}
