import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { projectTitle } from '../../../main';
import { AuthService } from '../../services/auth.service';
import { LoginModel } from '../../shared/models/login-model';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private title: Title
  ) { }

  login() {
    for (const i in this.loginForm.controls) {
      this.loginForm.controls[i].markAsDirty();
      this.loginForm.controls[i].updateValueAndValidity();
    }

    const loginModel: LoginModel = {
      email: this.loginForm.controls['email'].value,
      password: this.loginForm.controls['password'].value
    }

    this.subscription.add(this.authService.login(loginModel).subscribe({
      next: (response) => {
        if (response.result) {
          console.log('login succeeded');
        }
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('login failed');
        console.error(error);
      }
    }));
  }

  ngOnInit(): void {
    this.title.setTitle(`${projectTitle} - Login`);
    this.loginForm = this.fb.group({
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required]]
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
