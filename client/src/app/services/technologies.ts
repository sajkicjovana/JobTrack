import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Technology {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root'
})
export class Technologies {

  private apiUrl =
    'http://localhost:5254/api/Technologies';

  constructor(
    private http: HttpClient
  ) {}

  private getHeaders() {
    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }

  getAll(): Observable<Technology[]> {
    return this.http.get<Technology[]>(
      this.apiUrl,
      {
        headers: this.getHeaders()
      }
    );
  }

  create(name: string) {
    return this.http.post(
      this.apiUrl,
      { name },
      {
        headers: this.getHeaders()
      }
    );
  }

  update(id: number, name: string) {
    return this.http.put(
      `${this.apiUrl}/${id}`,
      { name },
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