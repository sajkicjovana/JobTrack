import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Topic {
  id: number;
  name: string;
  content: string;
  technologyId: number;
  technologyName: string;
  questionCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class TopicsService {

  private apiUrl =
    'http://localhost:5254/api/Topics';

  constructor(
    private http: HttpClient
  ) {}

  private getHeaders() {
    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }

  getAll(technologyId?: number): Observable<Topic[]> {
    let url = this.apiUrl;

    if (technologyId) {
      url += `?technologyId=${technologyId}`;
    }

    return this.http.get<Topic[]>(
      url,
      {
        headers: this.getHeaders()
      }
    );
  }

  getById(id: number): Observable<Topic> {
    return this.http.get<Topic>(
      `${this.apiUrl}/${id}`,
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