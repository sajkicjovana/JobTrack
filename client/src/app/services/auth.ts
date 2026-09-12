import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  email: string;
  role: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

@Injectable({
  providedIn: 'root'
})
export class Auth {

  private apiUrl =
    'http://localhost:5254/api/Auth';

  constructor(
    private http: HttpClient
  ) {}

  login(
    data: LoginRequest
  ): Observable<LoginResponse> {

    return this.http.post<LoginResponse>(
      `${this.apiUrl}/login`,
      data
    );
  }

  register(data: RegisterRequest) {
    return this.http.post(
      `${this.apiUrl}/register`,
      data
    );
  }

  saveSession(response: LoginResponse) {
    localStorage.setItem(
      'token',
      response.token
    );

    localStorage.setItem(
      'role',
      response.role
    );

    localStorage.setItem(
      'email',
      response.email
    );

    localStorage.setItem(
      'userId',
      String(response.userId)
    );
  }

  isLoggedIn(): boolean {
    return localStorage.getItem('token') !== null;
  }

  isAdmin(): boolean {
    return localStorage.getItem('role') === 'Admin';
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  getEmail(): string {
    return localStorage.getItem('email') ?? '';
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('email');
    localStorage.removeItem('userId');
  }
}