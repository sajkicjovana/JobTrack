import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders
} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class Questions {

  private apiUrl =
    'http://localhost:5254/api/Questions';

  constructor(
    private http: HttpClient
  ) {}

  private getHeaders() {
    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }

  getAll(topicId?: number): Observable<any[]> {
    let url = this.apiUrl;

    if (topicId) {
      url += `?topicId=${topicId}`;
    }

    return this.http.get<any[]>(
      url,
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