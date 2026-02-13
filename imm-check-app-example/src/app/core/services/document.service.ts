import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class DocumentService {
  private baseUrl = `${environment.apiUrl}/documents`;

  constructor(private http: HttpClient) {}

  // I-20
  getI20s(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/i20`);
  }

  getI20(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/i20/${id}`);
  }

  createI20(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/i20`, data);
  }

  // DS-2019
  getDS2019s(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/ds2019`);
  }

  getDS2019(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/ds2019/${id}`);
  }

  createDS2019(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/ds2019`, data);
  }

  // I-94
  getI94s(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/i94`);
  }

  getI94(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/i94/${id}`);
  }

  createI94(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/i94`, data);
  }

  // Passport
  getPassports(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/passport`);
  }

  getPassport(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/passport/${id}`);
  }

  createPassport(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/passport`, data);
  }

  // Visa
  getVisas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/visa`);
  }

  getVisa(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/visa/${id}`);
  }

  createVisa(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/visa`, data);
  }

  // Sponsored Student
  getSponsoredStudents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/sponsored-student`);
  }

  getSponsoredStudent(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/sponsored-student/${id}`);
  }

  createSponsoredStudent(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/sponsored-student`, data);
  }

  // Financial Support
  getFinancialSupports(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/financial-support`);
  }

  getFinancialSupport(id: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/financial-support/${id}`);
  }

  createFinancialSupport(data: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/financial-support`, data);
  }
}
