import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserService } from '../_services/user.service';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;

  private authWindow: Window;
  failed: boolean;
  error: string;
  errorDescription: string;
  isRequesting: boolean;



  ngOnInit() {
  }

  registerToggle() {
    this.registerMode = true;
  }

  cancelRegisterMode(registerMode: boolean) {
    this.registerMode = registerMode;
  }

}
