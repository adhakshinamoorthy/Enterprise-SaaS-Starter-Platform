# Contributing

Treat tenant isolation as a non-negotiable invariant. Every data-access change needs a cross-tenant negative test. Keep bounded-context dependencies explicit, version integration contracts, and document whether a capability is implemented, scaffolded, or proposed. Run both .NET and web CI checks before submitting changes.
