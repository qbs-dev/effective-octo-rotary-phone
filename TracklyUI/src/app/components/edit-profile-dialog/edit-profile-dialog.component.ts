import { Component, Inject } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { ProfileService } from 'src/app/services/profile.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { Profile, ProfileBase } from 'src/app/models/profile.model';
import { getErrorMessage } from 'src/app/helpers/custom-validators';

@Component({
  selector: 'app-edit-profile-dialog',
  templateUrl: './edit-profile-dialog.component.html',
  styleUrls: ['./edit-profile-dialog.component.less'],
})
export class EditProfileDialogComponent {
  isLoading: boolean = true;
  editProfileDetailsForm: FormGroup = null!;

  originalProfile: Profile = null!;

  firstName = new FormControl('', [
    Validators.required,
    Validators.minLength(2),
    Validators.maxLength(64),
  ]);
  middleName = new FormControl('', [
    Validators.minLength(2),
    Validators.maxLength(64),
  ]);
  lastName = new FormControl('', [
    Validators.minLength(2),
    Validators.maxLength(64),
  ]);
  getErrorMessage = getErrorMessage;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private formBuilder: FormBuilder,
    private errorService: HttpErrorResponseService,
    private profileService: ProfileService,
    private snack: SnackNotifyComponent,
    private dialogRef: MatDialogRef<EditProfileDialogComponent>
  ) {
    this.buildForm();
  }

  buildForm(): void {
    this.editProfileDetailsForm = this.formBuilder.group({
      firstName: this.firstName,
      middleName: this.middleName,
      lastName: this.lastName,
    });

    this.originalProfile = this.data.originalProfile as Profile;
    this.firstName.setValue(this.originalProfile.firstName);
    this.middleName.setValue(this.originalProfile.middleName);
    this.lastName.setValue(this.originalProfile.lastName);

    this.isLoading = false;
  }

  sendForm(): void {
    const userId = this.originalProfile.id;
    const newProfileDetails: ProfileBase = {
      firstName: this.firstName.value!,
      middleName: this.middleName.value!,
      lastName: this.lastName.value!,
    };
    this.isLoading = true;

    this.profileService
      .editProfileDetails(userId, newProfileDetails)
      .subscribe({
        next: (_) => {
          this.snack.openSnackBar('Profile details were changed', 'OK');
          this.dialogRef.close(true);
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
