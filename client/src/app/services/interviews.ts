import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class Interviews {

  private apiUrl =
    'http://localhost:5254/api/Interviews';

  constructor(
    private http: HttpClient
  ) {}

  private getHeaders() {
    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }

  getAll() {
    return this.http.get<any[]>(
      this.apiUrl,
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

  delete(id: number) {
    return this.http.delete(
      `${this.apiUrl}/${id}`,
      {
        headers: this.getHeaders()
      }
    );
  }
}