import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

import { Observable } from 'rxjs';


export interface DashboardInterview {
  id: number;
  interviewDate: string;
  type: string;
  contactPerson?: string;
  outcome?: string | null;

  companyName: string;
  position: string;
}


export interface DashboardTestResult {
  attemptId: number;
  testId: number;
  testTitle: string;
  technologyName: string;
  completedAt: string;
  correctAnswers: number;
  totalQuestions: number;
  percentage: number;
}


@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private apiUrl =
    'http://localhost:5254/api';


  constructor(
    private http: HttpClient
  ) {}


  getApplications(): Observable<any[]> {

    return this.http.get<any[]>(
      `${this.apiUrl}/JobApplications`,
      {
        headers: this.getHeaders()
      }
    );
  }


  getInterviews():
    Observable<DashboardInterview[]> {

    return this.http.get<DashboardInterview[]>(
      `${this.apiUrl}/Interviews`,
      {
        headers: this.getHeaders()
      }
    );
  }


  getResults():
    Observable<DashboardTestResult[]> {

    return this.http.get<DashboardTestResult[]>(
      `${this.apiUrl}/TestAttempts/my-results`,
      {
        headers: this.getHeaders()
      }
    );
  }


  private getHeaders(): HttpHeaders {

    const token =
      localStorage.getItem('token');

    return new HttpHeaders({
      Authorization: `Bearer ${token}`
    });
  }
}