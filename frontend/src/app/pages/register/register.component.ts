import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { projectTitle } from '../../../main';
import { AuthService } from '../../services/auth.service';
import { RegisterModel } from '../../shared/models/register-model';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {
  registerForm!: FormGroup;
  private subscription = new Subscription();

  constructor(
    private fb: FormBuilder,
    private title: Title,
    private authService: AuthService,
    private router: Router
  ) { }

  register() {
    for (const i in this.registerForm.controls) {
      this.registerForm.controls[i].markAsDirty();
      this.registerForm.controls[i].updateValueAndValidity();
    }

    const registerModel: RegisterModel = {
      fullName: this.registerForm.controls['fullName'].value,
      email: this.registerForm.controls['email'].value,
      password: this.registerForm.controls['password'].value,
      confirmPassword: this.registerForm.controls['confirmPassword'].value,
    }

    this.subscription.add(this.authService.register(registerModel).subscribe({
      next: (response) => {
        if (response.result) {
          console.log('registering succeeded');
        }
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('registering failed');
        console.error(error);
      }
    }));
  }

  ngOnInit(): void {
    this.title.setTitle(`${projectTitle} - Register`)
    this.registerForm = this.fb.group({
      fullName: [null, [Validators.required]],
      email: [null, [Validators.required, Validators.email]],
      password: [null, [Validators.required]],
      confirmPassword: [null, [Validators.required]],
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
