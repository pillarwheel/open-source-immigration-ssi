import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CredentialService } from 'src/app/core/services/credential.service';

@Component({
  selector: 'app-verify-credential',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="container mt-4">
      <h2>Verify Credential</h2>
      <p class="text-muted">Paste an SD-JWT credential to verify its claims</p>
      <a routerLink="/credentials" class="btn btn-sm btn-outline-secondary mb-3">Back to Dashboard</a>

      <div *ngIf="error" class="alert alert-danger">{{ error }}</div>

      <div class="card mb-3">
        <div class="card-body">
          <div class="mb-3">
            <label class="form-label">SD-JWT Credential</label>
            <textarea class="form-control font-monospace" rows="5" [(ngModel)]="credential"
                      placeholder="Paste the SD-JWT credential here (format: jwt~disclosure1~disclosure2~...)">
            </textarea>
          </div>
          <button class="btn btn-primary" (click)="verify()" [disabled]="verifying || !credential">
            {{ verifying ? 'Verifying...' : 'Verify' }}
          </button>
        </div>
      </div>

      <div *ngIf="result" class="card">
        <div class="card-header" [class.bg-success]="result.isValid" [class.bg-danger]="!result.isValid"
             [class.text-white]="true">
          {{ result.isValid ? 'Valid Credential' : 'Invalid Credential' }}
        </div>
        <div class="card-body">
          <div *ngIf="!result.isValid">
            <p class="text-danger"><strong>Error:</strong> {{ result.error }}</p>
          </div>
          <div *ngIf="result.isValid">
            <p><strong>Issuer:</strong> {{ result.issuerDid }}</p>
            <p><strong>Subject:</strong> {{ result.subjectDid }}</p>
            <p *ngIf="result.validFrom"><strong>Valid From:</strong> {{ result.validFrom | date:'short' }}</p>
            <p *ngIf="result.validUntil"><strong>Valid Until:</strong> {{ result.validUntil | date:'short' }}</p>

            <h5 class="mt-3">Disclosed Claims</h5>
            <table class="table table-sm table-striped" *ngIf="result.disclosedClaims">
              <thead>
                <tr>
                  <th>Claim</th>
                  <th>Value</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let claim of getClaimEntries(result.disclosedClaims)">
                  <td><code>{{ claim.key }}</code></td>
                  <td>{{ formatClaimValue(claim.value) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <div class="card mt-3">
        <div class="card-header">Verification Scenarios (OID4VP)</div>
        <div class="card-body">
          <p class="text-muted">Request a specific proof from a holder:</p>
          <div class="btn-group">
            <button class="btn btn-outline-primary" (click)="createPresentationRequest('f1-status')">
              Prove F-1 Status
            </button>
            <button class="btn btn-outline-primary" (click)="createPresentationRequest('financial-support')">
              Prove Financial Support
            </button>
          </div>
          <div *ngIf="presentationRequest" class="mt-3">
            <label class="form-label"><strong>Presentation Request (send to wallet):</strong></label>
            <textarea class="form-control font-monospace" rows="5" readonly
                      [value]="presentationRequestJson"></textarea>
          </div>
        </div>
      </div>
    </div>
  `
})
export class VerifyCredentialComponent {
  credential = '';
  verifying = false;
  error = '';
  result: any = null;
  presentationRequest: any = null;
  presentationRequestJson = '';

  constructor(private credentialService: CredentialService) {}

  verify() {
    this.verifying = true;
    this.error = '';
    this.result = null;

    this.credentialService.verifyCredential(this.credential.trim()).subscribe({
      next: (res) => {
        this.result = res;
        this.verifying = false;
      },
      error: (err: any) => {
        this.error = err.error?.error || 'Verification failed';
        this.verifying = false;
      }
    });
  }

  createPresentationRequest(scenario: string) {
    this.credentialService.createPresentationRequest(scenario).subscribe({
      next: (req) => {
        this.presentationRequest = req;
        this.presentationRequestJson = JSON.stringify(req, null, 2);
      },
      error: (err: any) => this.error = err.error?.error || 'Failed to create request'
    });
  }

  getClaimEntries(claims: Record<string, any>): { key: string; value: any }[] {
    return Object.entries(claims)
      .filter(([k]) => k !== '_sd')
      .map(([key, value]) => ({ key, value }));
  }

  formatClaimValue(value: any): string {
    if (typeof value === 'object') return JSON.stringify(value);
    return String(value);
  }
}
