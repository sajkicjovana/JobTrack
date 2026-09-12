import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  Router,
  RouterLink
} from '@angular/router';

import { Auth } from '../../services/auth';

@Component({
  selector: 'app-register',
  imports: [
    FormsModule,
    RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {

  firstName = '';
  lastName = '';
  email = '';
  password = '';
  confirmPassword = '';

  loading = false;
  errorMessage = '';

  constructor(
    private authService: Auth,
    private router: Router
  ) {}

  onRegister() {

    this.errorMessage = '';

    if (this.password !== this.confirmPassword) {
      this.errorMessage =
        'Passwords do not match.';
      return;
    }

    this.loading = true;

    this.authService.register({
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      password: this.password
    }).subscribe({
      next: () => {
        this.loading = false;

        this.router.navigate([
          '/login'
        ]);
      },

      error: (error) => {
        this.loading = false;

        this.errorMessage =
          error.error?.message ||
          error.error ||
          'Registration failed.';
      }
    });
  }

  showPassword = false;
  showConfirmPassword = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword =
      !this.showConfirmPassword;
  }
}