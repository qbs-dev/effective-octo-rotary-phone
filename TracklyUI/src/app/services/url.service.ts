import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MessageResponse, Page } from '../models/common.model';
import {
  Url,
  UrlEditRequest,
  UrlShort,
  UrlStatsResponse,
  UrlVisit,
  VisitsByCountry,
  VisitsByIpAddress,
} from '../models/url.model';
import { AuthService } from './auth.service';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

const endpoint = '/api/urls';

@Injectable({
  providedIn: 'root',
})
export class UrlService {
  constructor(private http: HttpClient, private authService: AuthService) {}

  getUrlsPage(
    sorts: string,
    page: number,
    pageSize: number
  ): Observable<Page<UrlShort>> {
    return this.http.get<Page<UrlShort>>(
      endpoint +
        `/user/${this.authService.getUserId()}` +
        `?page=${page}&pageSize=${pageSize}` +
        `&sorts=${sorts}`
    );
  }

  getUrlVisitsPage(
    urlId: number,
    filters: string,
    sorts: string,
    page: number,
    pageSize: number
  ): Observable<Page<UrlVisit>> {
    return this.http.get<Page<UrlVisit>>(
      endpoint +
        `/${urlId}/visits` +
        `?userId=${this.authService.getUserId()}` +
        `&page=${page}&pageSize=${pageSize}` +
        `&filters=${filters}&sorts=${sorts}`
    );
  }

  getUrlVisitsByCountry(
    urlId: number,
    startDate: Date,
    endDate: Date,
    limit: number
  ): Observable<UrlStatsResponse<VisitsByCountry>> {
    return this.http.get<UrlStatsResponse<VisitsByCountry>>(
      endpoint +
        `/${urlId}/stats/country` +
        `?userId=${this.authService.getUserId()}` +
        `&startDateUtc=${startDate}&endDateUtc=${endDate}` +
        `&limit=${limit}`
    );
  }

  getUrlVisitsByIpAddress(
    urlId: number,
    startDate: Date,
    endDate: Date,
    limit: number
  ): Observable<UrlStatsResponse<VisitsByIpAddress>> {
    return this.http.get<UrlStatsResponse<VisitsByIpAddress>>(
      endpoint +
        `/${urlId}/stats/ip-address` +
        `?userId=${this.authService.getUserId()}` +
        `&startDateUtc=${startDate}&endDateUtc=${endDate}` +
        `&limit=${limit}`
    );
  }

  getUrlDetails(urlId: number): Observable<Url> {
    return this.http.get<Url>(
      endpoint + `/${urlId}?userId=${this.authService.getUserId()}`
    );
  }

  createUrl(newUrlRequest: UrlEditRequest): Observable<Url> {
    return this.http.post<Url>(
      endpoint + `/create?userId=${this.authService.getUserId()}`,
      newUrlRequest,
      httpOptions
    );
  }

  editUrlDetails(editUrlRequest: UrlEditRequest): Observable<Url> {
    return this.http.post<Url>(
      endpoint + `/edit?userId=${this.authService.getUserId()}`,
      editUrlRequest,
      httpOptions
    );
  }

  deleteUrl(urlId: number): Observable<MessageResponse> {
    return this.http.delete<MessageResponse>(
      endpoint + `/${urlId}?userId=${this.authService.getUserId()}`,
      httpOptions
    );
  }

  findUrlByPath(urlPath: string): Observable<Url> {
    return this.http.get<Url>(
      endpoint + `/find/${urlPath}` + `?userId=${this.authService.getUserId()}`
    );
  }
}
