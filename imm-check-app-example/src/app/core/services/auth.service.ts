import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface AuthResponse {
  token: string;
  email: string;
  role: string;
  expiration: string;
}

export interface UserContext {
  email: string;
  role: string;
  token: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private currentUser = new BehaviorSubject<UserContext | null>(this.loadUser());
  currentUser$ = this.currentUser.asObservable();

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, { email, password })
      .pipe(tap(res => this.setUser(res)));
  }

  register(email: string, password: string, firstName: string, lastName: string, role: string): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/register`, {
      email, password, firstName, lastName, role
    });
  }

  logout(): void {
    localStorage.removeItem('auth_user');
    this.currentUser.next(null);
  }

  get isLoggedIn(): boolean {
    return !!this.currentUser.value;
  }

  get token(): string | null {
    return this.currentUser.value?.token ?? null;
  }

  get role(): string | null {
    return this.currentUser.value?.role ?? null;
  }

  private setUser(res: AuthResponse): void {
    const user: UserContext = { email: res.email, role: res.role, token: res.token };
    localStorage.setItem('auth_user', JSON.stringify(user));
    this.currentUser.next(user);
  }

  private loadUser(): UserContext | null {
    const stored = localStorage.getItem('auth_user');
    return stored ? JSON.parse(stored) : null;
  }
}
