import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TestAttempts {

  private apiUrl =
    'http://localhost:5254/api/TestAttempts';

  constructor(
    private http: HttpClient
  ) {}

  private getHeaders() {
    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }

  start(testId: number): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/start/${testId}`,
      {},
      {
        headers: this.getHeaders()
      }
    );
  }

  submit(
    attemptId: number,
    answers: any[]
  ): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/${attemptId}/submit`,
      { answers },
      {
        headers: this.getHeaders()
      }
    );
  }

  getMyResults(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/my-results`,
      {
        headers: this.getHeaders()
      }
    );
  }

  getResult(attemptId: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/${attemptId}/result`,
      {
        headers: this.getHeaders()
      }
    );
  }
}