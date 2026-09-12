import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Tests {

  private apiUrl =
    'http://localhost:5254/api/Tests';

  constructor(
    private http: HttpClient
  ) {}

  private getHeaders() {
    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }

  getPublished(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/published`,
      {
        headers: this.getHeaders()
      }
    );
  }

  getAllAdmin(): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/admin`,
      {
        headers: this.getHeaders()
      }
    );
  }

  getAdminById(id: number): Observable<any> {
    return this.http.get<any>(
      `${this.apiUrl}/admin/${id}`,
      {
        headers: this.getHeaders()
      }
    );
  }

  create(data: any) {
    return this.http.post(
      this.apiUrl,
      data,
      {
        headers: this.getHeaders()
      }
    );
  }

  update(id: number, data: any) {
    return this.http.put(
      `${this.apiUrl}/${id}`,
      data,
      {
        headers: this.getHeaders()
      }
    );
  }

  publish(id: number) {
    return this.http.patch(
      `${this.apiUrl}/${id}/publish`,
      {},
      {
        headers: this.getHeaders()
      }
    );
  }

  unpublish(id: number) {
    return this.http.patch(
      `${this.apiUrl}/${id}/unpublish`,
      {},
      {
        headers: this.getHeaders()
      }
    );
  }

  delete(id: number) {
    return this.http.delete(
      `${this.apiUrl}/${id}`,
      {
        headers: this.getHeaders()
      }
    );
  }
}