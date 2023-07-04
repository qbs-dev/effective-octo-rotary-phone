import { Component, OnInit } from '@angular/core';
import { UrlService } from 'src/app/services/url.service';
import { SnackNotifyComponent } from '../snack-notify/snack-notify.component';
import { HttpErrorResponseService } from 'src/app/services/http-error-response.service';
import { ActivatedRoute } from '@angular/router';
import {
  UrlStatsResponse,
  VisitsByCountry,
  VisitsByIpAddress,
} from 'src/app/models/url.model';
import { PieChartOptions } from 'src/app/models/common.model';

@Component({
  selector: 'app-url-stats',
  templateUrl: './url-stats.component.html',
  styleUrls: ['./url-stats.component.less'],
})
export class UrlStatsComponent implements OnInit {
  isIpAddressError: boolean = false;
  isCountryError: boolean = false;
  isUrlVisitsByIpAddressLoading: boolean = true;
  isUrlVisitsByCountryLoading: boolean = true;
  urlId: number = null!;

  visitsByIpAddressChartOptions?: PieChartOptions;
  visitsByCountryChartOptions?: PieChartOptions;

  constructor(
    private urlService: UrlService,
    private snack: SnackNotifyComponent,
    private errorService: HttpErrorResponseService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.urlId = this.route.snapshot.params['url-id'];

    this.getUrlVisitsByIpAddressChart(this.urlId);
    this.getUrlVisitsByCountryChart(this.urlId);
  }

  getUrlVisitsByIpAddressChart(urlId: number) {
    this.urlService.getUrlVisitsByIpAddress(urlId).subscribe({
      next: (res: UrlStatsResponse<VisitsByIpAddress>) => {
        this.isUrlVisitsByIpAddressLoading = false;
        this.isIpAddressError = false;
        this.buildVisitsByIpAddressChart(res);
      },
      error: (error) => {
        this.snack.openSnackBar(
          'Error: ' + this.errorService.getStringFromError(error),
          'OK'
        );
        this.isUrlVisitsByIpAddressLoading = false;
        this.isIpAddressError = true;
      },
    });
  }

  getUrlVisitsByCountryChart(urlId: number) {
    this.urlService.getUrlVisitsByCountry(urlId).subscribe({
      next: (res: UrlStatsResponse<VisitsByCountry>) => {
        this.isUrlVisitsByCountryLoading = false;
        this.isCountryError = false;
        this.buildVisitsByCountryChart(res);
      },
      error: (error) => {
        this.snack.openSnackBar(
          'Error: ' + this.errorService.getStringFromError(error),
          'OK'
        );
        this.isUrlVisitsByCountryLoading = false;
        this.isCountryError = true;
      },
    });
  }

  buildVisitsByIpAddressChart(res: UrlStatsResponse<VisitsByIpAddress>) {
    this.visitsByIpAddressChartOptions = {
      labelData: res.stats.map((x) => x.ipAddress),
      datasetData: res.stats.map((x) => x.visitsCount),
      datasetLabel: 'Visits',
    };
  }

  buildVisitsByCountryChart(res: UrlStatsResponse<VisitsByCountry>) {
    this.visitsByCountryChartOptions = {
      labelData: res.stats.map((x) => x.countryCode),
      datasetData: res.stats.map((x) => x.visitsCount),
      datasetLabel: 'Visits',
    };
  }
}
