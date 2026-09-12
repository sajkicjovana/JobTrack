import { Injectable } from '@angular/core';

import {
  HttpClient,
  HttpHeaders,
  HttpParams
} from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class JobApplications {

  private apiUrl =
    'http://localhost:5254/api/JobApplications';


  constructor(
    private http: HttpClient
  ) {}


  private getHeaders() {

    return new HttpHeaders().set(
      'Authorization',
      `Bearer ${localStorage.getItem('token')}`
    );
  }


  getAll(
    search: string = '',
    status: string = '',
    technologyId: number | null = null
  ) {

    let params = new HttpParams();


    if (search.trim()) {
      params = params.set(
        'search',
        search.trim()
      );
    }


    if (status) {
      params = params.set(
        'status',
        status
      );
    }


    if (technologyId != null) {
      params = params.set(
        'technologyId',
        technologyId.toString()
      );
    }


    return this.http.get<any[]>(
      this.apiUrl,
      {
        headers: this.getHeaders(),
        params
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


  update(
    id: number,
    data: any
  ) {

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