import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { UrlService } from 'src/app/services/url.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { Url } from 'src/app/models/url.model';
import { EditUrlDialogComponent } from '../edit-url-dialog/edit-url-dialog.component';

@Component({
  selector: 'app-url-details',
  templateUrl: './url-details.component.html',
  styleUrls: ['./url-details.component.less'],
})
export class UrlDetailsComponent implements OnInit, OnDestroy {
  url: Url = null!;
  isError: boolean = false;
  isLoading: boolean = true;
  errorText: string = '';

  urlIdRouteParam$: Subscription = null!;

  constructor(
    private urlService: UrlService,
    private errorService: HttpErrorResponseService,
    private route: ActivatedRoute,
    private snack: SnackNotifyComponent,
    public editUrlDialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.urlIdRouteParam$ = this.route.params.subscribe((params) => {
      this.isLoading = true;
      this.isError = false;
      this.getUrlDetails(params['url-id']);
    });
  }

  ngOnDestroy(): void {
    this.urlIdRouteParam$.unsubscribe();
  }

  getUrlDetails(urlIdString: string) {
    const urlId = Number.parseInt(urlIdString);
    this.urlService.getUrlDetails(urlId).subscribe({
      next: (result: Url) => {
        this.url = result;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorText = this.errorService.getStringFromError(error);
        this.isError = true;
        this.isLoading = false;
      },
    });
  }

  openEditUrlDialog() {
    this.editUrlDialog.open(EditUrlDialogComponent, {
      data: {
        isAdding: false,
        originalUrl: this.url,
      },
      maxWidth: '90vw',
      maxHeight: '90vh',
    });
  }
}
