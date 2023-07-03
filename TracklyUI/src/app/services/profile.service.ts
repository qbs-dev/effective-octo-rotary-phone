import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Profile, ProfileBase, RegisterRequest } from '../models/profile.model';
import { Observable } from 'rxjs';
import { MessageResponse } from '../models/common.model';
import { sha3_256 } from '@noble/hashes/sha3';
import { bytesToHex } from '@noble/hashes/utils';

const endpoint = '/api/profile';
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
};

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  constructor(private http: HttpClient) {}

  register(regReq: RegisterRequest): Observable<MessageResponse> {
    regReq.password = bytesToHex(sha3_256(regReq.password));
    return this.http.post<MessageResponse>(
      endpoint + '/register',
      regReq,
      httpOptions
    );
  }

  getProfileDetails(userId: number): Observable<Profile> {
    return this.http.get<Profile>(endpoint + `/${userId}`, httpOptions);
  }

  editProfileDetails(
    userId: number,
    editUserDetails: ProfileBase
  ): Observable<Profile> {
    return this.http.post<Profile>(
      endpoint + `/${userId}/edit`,
      editUserDetails,
      httpOptions
    );
  }

  deleteProfile(userId: number): Observable<MessageResponse> {
    return this.http.delete<MessageResponse>(
      endpoint + `/${userId}`,
      httpOptions
    );
  }
}
