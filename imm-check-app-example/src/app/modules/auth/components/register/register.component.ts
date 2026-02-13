import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="container mt-5">
      <div class="row justify-content-center">
        <div class="col-md-6">
          <div class="card">
            <div class="card-header"><h3>Register</h3></div>
            <div class="card-body">
              <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
              <div *ngIf="success" class="alert alert-success">{{ success }}</div>
              <form (ngSubmit)="onSubmit()">
                <div class="mb-3">
                  <label class="form-label">First Name</label>
                  <input type="text" class="form-control" [(ngModel)]="firstName" name="firstName">
                </div>
                <div class="mb-3">
                  <label class="form-label">Last Name</label>
                  <input type="text" class="form-control" [(ngModel)]="lastName" name="lastName">
                </div>
                <div class="mb-3">
                  <label class="form-label">Email</label>
                  <input type="email" class="form-control" [(ngModel)]="email" name="email" required>
                </div>
                <div class="mb-3">
                  <label class="form-label">Password</label>
                  <input type="password" class="form-control" [(ngModel)]="password" name="password" required>
                </div>
                <div class="mb-3">
                  <label class="form-label">Role</label>
                  <select class="form-select" [(ngModel)]="role" name="role">
                    <option value="Student">Student</option>
                    <option value="DSO">DSO (Designated School Official)</option>
                    <option value="Advisor">Advisor</option>
                    <option value="Admin">Admin</option>
                  </select>
                </div>
                <button type="submit" class="btn btn-primary w-100">Register</button>
              </form>
              <p class="mt-3 text-center">
                Already have an account? <a routerLink="/login">Login</a>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  email = '';
  password = '';
  firstName = '';
  lastName = '';
  role = 'Student';
  error = '';
  success = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.error = '';
    this.success = '';
    this.authService.register(this.email, this.password, this.firstName, this.lastName, this.role).subscribe({
      next: () => {
        this.success = 'Registration successful! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err: any) => this.error = err.error?.errors?.join(', ') || 'Registration failed'
    });
  }
}
