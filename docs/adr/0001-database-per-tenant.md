# ADR 0001: Database per tenant

Status: Accepted for this reference

Database-per-tenant provides a strong isolation and recovery boundary at the cost of provisioning, migration, connection-pool, and fleet-management complexity. Small tenants may require a pooled alternative. The platform must automate schema-version inventory, rollout waves, backup restore tests, and tenant offboarding before production.
