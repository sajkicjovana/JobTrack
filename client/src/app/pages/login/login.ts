import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  Router,
  RouterLink
} from '@angular/router';

import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {

  email = '';
  password = '';

  loading = false;
  errorMessage = '';

  constructor(
    private authService: Auth,
    private router: Router
  ) {}

  onLogin() {

    this.errorMessage = '';
    this.loading = true;

    this.authService.login({
      email: this.email,
      password: this.password
    }).subscribe({
      next: (response) => {

        this.authService.saveSession(
          response
        );

        this.loading = false;

        if (response.role === 'Admin') {
          this.router.navigate([
            '/admin'
          ]);
        } else {
          this.router.navigate([
            '/dashboard'
          ]);
        }
      },

      error: () => {

        this.loading = false;

        this.errorMessage =
          'Invalid email or password.';
      }
    });
  }

  showPassword = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }
}