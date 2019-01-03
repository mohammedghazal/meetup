import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '../../../node_modules/@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getReportedUsers() {
    return this.http.get(this.baseUrl + 'admin/usersWith');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
  }

  getPhotosForApproval() {
    return this.http.get(this.baseUrl + 'admin/photosForModeration');
  }

  approvePhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/approvePhoto/' + photoId, {});
  }

  rejectPhoto(photoId) {
    return this.http.post(this.baseUrl + 'admin/rejectPhoto/' + photoId, {});
  }
}