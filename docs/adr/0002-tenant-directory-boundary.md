# ADR 0002: Keep tenant directory behind a replaceable boundary

Status: Accepted

`ITenantStore` separates resolution from storage. Local development reads configured tenants so the API is runnable without coupling the tenant service to the Catalog database. Production should use a protected tenant-directory adapter and secret references, with caching, revocation, audit, and failure-mode design.
