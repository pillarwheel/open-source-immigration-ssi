# ImmCheck Demo Script

Step-by-step walkthrough demonstrating the full SSI credential lifecycle for immigration documents.

## Prerequisites

Start the API:
```bash
cd src
dotnet run --project ImmCheck.Api
```

The API will be available at `http://localhost:5000` with Swagger UI at `http://localhost:5000/swagger`.

---

## Step 1: Verify the system is running

```bash
# Check Swagger is accessible
curl http://localhost:5000/swagger/v1/swagger.json | head -c 200

# List supported DID methods
curl http://localhost:5000/api/did/methods
# Expected: {"resolve":["key","web","prism","cheqd"],"create":["key","prism"]}
```

## Step 2: Create a DID for the institution (DSO)

```bash
# Create a did:key for the issuing institution
curl -X POST http://localhost:5000/api/did/create \
  -H "Content-Type: application/json" \
  -d '{"method": "key"}'
# Returns: { "id": "did:key:z6Mk...", "verificationMethod": [...] }
```

Save the DID — this is the institution's issuer identity.

## Step 3: Resolve the DID

```bash
# Resolve the DID to see its full DID Document
curl http://localhost:5000/api/did/resolve/did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK
# Returns: full W3C DID Document with verification methods
```

## Step 4: View available credential schemas

```bash
curl http://localhost:5000/api/credential/schemas
# Returns: I20Credential and FinancialSupportCredential schemas
# with required claims and selective disclosure fields
```

## Step 5: DSO issues an I-20 Verifiable Credential

```bash
curl -X POST http://localhost:5000/api/credential/issue \
  -H "Content-Type: application/json" \
  -d '{
    "issuerDid": "did:key:z6MkDSOIssuer",
    "subjectDid": "did:key:z6MkStudentAlice",
    "credentialType": "I20Credential",
    "validityDays": 365,
    "claims": {
      "sevisId": "N0001234567",
      "studentName": "Alice Johnson",
      "programStatus": "Active",
      "educationLevel": "Master'\''s",
      "primaryMajor": "Computer Science",
      "programStartDate": "2024-08-15",
      "programEndDate": "2026-05-15",
      "institutionName": "International University"
    }
  }'
# Returns:
# {
#   "credentialId": "urn:uuid:...",
#   "format": "vc+sd-jwt",
#   "serializedCredential": "eyJ0eXAi...~WyJhYmMx...~WyJkZWYy...~"
# }
```

Note the `~` separators in the serialized credential — these separate the JWT from the selective disclosure elements.

## Step 6: Verify the issued credential

```bash
# Copy the serializedCredential from Step 5
curl -X POST http://localhost:5000/api/credential/verify \
  -H "Content-Type: application/json" \
  -d '{"serializedCredential": "eyJ0eXAi...~WyJhYmMx...~"}'
# Returns:
# {
#   "isValid": true,
#   "issuerDid": "did:key:z6MkDSOIssuer",
#   "disclosedClaims": { "sevisId": "N0001234567", "studentName": "Alice Johnson", ... },
#   "validFrom": "2024-...",
#   "validUntil": "2025-..."
# }
```

## Step 7: Selective Disclosure — prove enrollment without revealing SEVIS ID

The SD-JWT contains multiple `~`-separated disclosures. By presenting only some disclosures, the student reveals only selected claims.

```bash
# Take the JWT (before first ~) and only the disclosures for
# non-sensitive claims (programStatus, studentName, institutionName)
# Omit the disclosure for sevisId

# The verifier sees: studentName, programStatus, institutionName
# The verifier does NOT see: sevisId, educationLevel, dates
```

This is the core privacy feature — students prove they are in valid F-1 status without revealing their SEVIS ID.

## Step 8: OID4VCI Flow — Standards-compliant issuance

```bash
# Step 8a: Create credential offer
curl -X POST http://localhost:5000/api/oid4vci/credential-offer \
  -H "Content-Type: application/json" \
  -d '{
    "issuerDid": "did:key:z6MkDSOIssuer",
    "subjectDid": "did:key:z6MkStudentBob",
    "credentialConfigurationId": "I20Credential",
    "claims": {
      "sevisId": "N0002345678",
      "studentName": "Bob Smith",
      "programStatus": "Active",
      "educationLevel": "PhD",
      "primaryMajor": "Physics",
      "programStartDate": "2024-08-15",
      "programEndDate": "2029-05-15",
      "institutionName": "Pacific Coast University"
    },
    "validityDays": 365
  }'
# Returns: credential offer with pre-authorized code

# Step 8b: Exchange pre-auth code for access token
curl -X POST http://localhost:5000/api/oid4vci/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=urn:ietf:params:oauth:grant-type:pre-authorized_code&pre-authorized_code=CODE_FROM_STEP_8A"
# Returns: { "access_token": "...", "token_type": "Bearer" }

# Step 8c: Request credential with access token
curl -X POST http://localhost:5000/api/oid4vci/credential \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer TOKEN_FROM_STEP_8B" \
  -d '{
    "format": "vc+sd-jwt",
    "credential_definition": {
      "type": ["VerifiableCredential", "I20Credential"]
    }
  }'
# Returns: issued SD-JWT credential
```

## Step 9: OID4VP Flow — Verifier requests proof of F-1 status

```bash
# Step 9a: See available verification scenarios
curl http://localhost:5000/api/oid4vp/scenarios
# Returns: f1-status and financial-support scenarios

# Step 9b: Create presentation request
curl -X POST http://localhost:5000/api/oid4vp/request \
  -H "Content-Type: application/json" \
  -d '{"scenario": "f1-status"}'
# Returns: presentation request with nonce and definition

# Step 9c: Submit presentation (student responds with their credential)
curl -X POST http://localhost:5000/api/oid4vp/response \
  -H "Content-Type: application/json" \
  -d '{
    "vp_token": "THE_SD_JWT_CREDENTIAL",
    "state": "STATE_FROM_STEP_9B",
    "presentation_submission": {
      "id": "submission-1",
      "definition_id": "f1-status-verification",
      "descriptor_map": [
        {"id": "i20-credential", "format": "vc+sd-jwt", "path": "$"}
      ]
    }
  }'
# Returns: { "isValid": true, ... }

# Step 9d: Check verification status
curl http://localhost:5000/api/oid4vp/status/STATE_FROM_STEP_9B
# Returns: { "status": "completed", "isValid": true }
```

## Step 10: Revoke a credential

```bash
# Revoke the credential (e.g., student transfers to another institution)
curl -X POST http://localhost:5000/api/credential/CREDENTIAL_ID/revoke

# Check the status list — the revocation bit is set
curl http://localhost:5000/api/credential/status-list/did:key:z6MkDSOIssuer
# Returns: { "encodedList": "base64...", "totalEntries": 16384 }
```

---

## Step 11: Publish a DID to Cardano (requires Docker)

With `docker-compose up` running (for Identus Cloud Agent):

```bash
# Create a did:prism DID
curl -X POST http://localhost:5000/api/did/create \
  -H "Content-Type: application/json" \
  -d '{"method": "prism"}'
# Returns: did:prism:... (unpublished, long-form DID)

# Publish the DID to Cardano preprod
curl -X POST http://localhost:5000/api/did/publish/did:prism:COPY_DID_FROM_ABOVE
# Returns: { "success": true, "status": "publication_pending", "scheduledOperation": "..." }

# Check publication status
curl http://localhost:5000/api/did/status/did:prism:COPY_DID_FROM_ABOVE
# Returns: { "did": "...", "status": "PUBLICATION_PENDING", "blockchain": "cardano", "network": "preprod" }
# After ~2 minutes (Cardano block confirmation):
# Returns: { "did": "...", "status": "PUBLISHED", "blockchain": "cardano", "network": "preprod" }
```

## Step 12: Resolve a did:cheqd identifier (multi-blockchain)

```bash
# Resolve a cheqd DID (uses cheqd's public resolver)
curl http://localhost:5000/api/did/resolve/did:cheqd:mainnet:zF7rhDBfUt9d1gJPjx7s1JXfUY7oVWkY
# Returns: W3C DID Document with verification methods from cheqd blockchain
```

---

## Summary of what was demonstrated

1. **DID Creation & Resolution** — Institutions get decentralized identifiers
2. **Credential Issuance** — DSOs issue SD-JWT Verifiable Credentials for I-20 documents (now signed with Ed25519)
3. **Selective Disclosure** — Students control which claims are revealed
4. **OID4VCI** — Standards-compliant credential issuance via pre-authorized code flow
5. **OID4VP** — Standards-compliant presentation exchange with predefined verification scenarios
6. **Revocation** — Credentials can be revoked with status list updates
7. **Cardano Publication** — did:prism DIDs published to Cardano preprod testnet
8. **Multi-Blockchain** — did:cheqd resolution demonstrates support beyond Cardano
