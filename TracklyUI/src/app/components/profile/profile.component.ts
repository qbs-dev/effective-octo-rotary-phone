import { Component, OnInit } from '@angular/core';
import { Profile, ProfileBase } from 'src/app/models/profile.model';
import { AuthService } from 'src/app/services/auth.service';
import { ProfileService } from 'src/app/services/profile.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { EditProfileDialogComponent } from '../edit-profile-dialog/edit-profile-dialog.component';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.less'],
})
export class ProfileComponent implements OnInit {
  isLoading = true;
  isError = false;
  profile: Profile = null!;

  constructor(
    private authService: AuthService,
    private profileService: ProfileService,
    private snack: SnackNotifyComponent,
    private errorService: HttpErrorResponseService,
    private editProfileDetailsDialog: MatDialog,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProfileDetails();
  }

  loadProfileDetails() {
    this.isLoading = true;
    this.profileService
      .getProfileDetails(this.authService.getUserId())
      .subscribe({
        next: (result: Profile) => {
          this.profile = result;
          this.isLoading = false;
        },
        error: (error) => {
          this.snack.openSnackBar(
            'Error: ' + this.errorService.getStringFromError(error),
            'OK'
          );
          this.isError = true;
          this.isLoading = false;
        },
      });
  }

  openEditProfileDetailsDialog() {
    const editProfileDialogRef = this.editProfileDetailsDialog.open(
      EditProfileDialogComponent,
      {
        data: {
          originalProfile: this.profile,
        },
        maxWidth: '90vw',
        maxHeight: '90vh',
      }
    );

    editProfileDialogRef.afterClosed().subscribe({
      next: (res?: boolean) => {
        if (res) {
          this.loadProfileDetails();
        }
      },
    });
  }

  getFullName(profile: ProfileBase) {
    return (
      profile.firstName +
      (profile.middleName ? ' ' + profile.middleName : '') +
      (profile.lastName ? ' ' + profile.lastName : '')
    );
  }
}
