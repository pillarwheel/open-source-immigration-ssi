import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CredentialService } from 'src/app/core/services/credential.service';

@Component({
  selector: 'app-credential-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="container mt-4">
      <h2>Credential Wallet</h2>
      <p class="text-muted">Your issued Verifiable Credentials</p>

      <div class="row mb-3">
        <div class="col">
          <div class="btn-group">
            <a class="btn btn-primary" routerLink="/credentials/issue">Issue New Credential</a>
            <a class="btn btn-outline-secondary" routerLink="/credentials/verify">Verify Credential</a>
          </div>
        </div>
      </div>

      <div *ngIf="loading" class="text-center py-4">
        <div class="spinner-border" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>

      <div *ngIf="error" class="alert alert-danger">{{ error }}</div>

      <div *ngIf="!loading && credentials.length === 0" class="alert alert-info">
        No credentials found. Issue a credential to get started.
      </div>

      <div class="row">
        <div class="col-md-6 mb-3" *ngFor="let cred of credentials">
          <div class="card" [class.border-danger]="cred.isRevoked">
            <div class="card-header d-flex justify-content-between">
              <span class="badge" [class.bg-success]="!cred.isRevoked" [class.bg-danger]="cred.isRevoked">
                {{ cred.isRevoked ? 'Revoked' : 'Active' }}
              </span>
              <small class="text-muted">{{ cred.credentialType }}</small>
            </div>
            <div class="card-body">
              <h6 class="card-title">{{ formatType(cred.credentialType) }}</h6>
              <p class="card-text">
                <small>
                  <strong>Issuer:</strong> {{ truncateDid(cred.issuerDid) }}<br>
                  <strong>Issued:</strong> {{ cred.issuedAt | date:'short' }}<br>
                  <strong>Expires:</strong> {{ cred.expiresAt ? (cred.expiresAt | date:'short') : 'Never' }}
                </small>
              </p>
              <button class="btn btn-sm btn-outline-danger" *ngIf="!cred.isRevoked"
                      (click)="revoke(cred.id)">
                Revoke
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class CredentialDashboardComponent implements OnInit {
  credentials: any[] = [];
  loading = true;
  error = '';

  constructor(private credentialService: CredentialService) {}

  ngOnInit() {
    // For demo, load credentials for a test subject DID
    this.credentialService.getCredentialsBySubject('did:key:z6MkStudent').subscribe({
      next: (creds) => {
        this.credentials = creds;
        this.loading = false;
      },
      error: () => {
        this.credentials = [];
        this.loading = false;
      }
    });
  }

  revoke(id: string) {
    this.credentialService.revokeCredential(id).subscribe({
      next: () => {
        const cred = this.credentials.find(c => c.id === id);
        if (cred) cred.isRevoked = true;
      },
      error: (err: any) => this.error = err.error?.error || 'Revocation failed'
    });
  }

  formatType(type: string): string {
    return type.replace(/([A-Z])/g, ' $1').trim();
  }

  truncateDid(did: string): string {
    if (!did || did.length < 30) return did;
    return did.substring(0, 20) + '...' + did.substring(did.length - 8);
  }
}
