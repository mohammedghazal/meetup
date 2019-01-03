import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { AdminService } from '../../_services/admin.service';
import { BsModalService, BsModalRef } from '../../../../node_modules/ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { AuthService } from '../../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from '../../_models/pagination';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService,
    private modalService: BsModalService, private userService: UserService,
    private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getReportedUsers();
  }

  getReportedUsers() {
    this.adminService.getReportedUsers().subscribe((users: User[]) => {
      this.users = users;
    }, error => {
      console.log(error);
    });
  }


  deleteUser(id: number) {
    this.alertify.confirm('Are you sure you want to delete this user?', () => {
      this.userService.deleteUser(id).subscribe(() => {
        this.users.splice(this.users.findIndex(p => p.id === id), 1);
        this.alertify.success('the user has been deleted');
      }, error => {
        this.alertify.error('Failed to delete the user');
      });
    });
  }

}
