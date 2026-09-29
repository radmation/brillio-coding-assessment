# Listing scorer

Used only when `targetBudget` is greater than 0. Other filters still run first. This module **reorders** the filtered list (and sets `score`) **before** pagination.

Implementation: `ListingScorer.cs`.

## Score (0 to 1)

```
score = (0.7 × priceScore) + (0.3 × recencyScore)
```

Weights are `PriceWeight` and `RecencyWeight` at the top of `ListingScorer`.

**Price closeness** — exact match is 1; farther prices decay:

```
priceDelta = |listPrice − targetBudget| / targetBudget
priceScore = 1 / (1 + priceDelta)
```

Over and under budget are treated the same (absolute difference).

**Recency** — listed today is 1; older listings decay. Days in the future are clamped to 0:

```
days = max(0, utcToday − listedDate)
recencyScore = 1 / (1 + days)
```

A small price miss can lose to a newer listing, because recency is 30% of the score.

## Rank order

1. Higher `score` first  
2. Newer `listedDate`  
3. `Id` (ordinal) for a stable tie-break  

`Rank` can take an optional `utcNow` so tests do not depend on the clock.

## Possible adjustments

These match the notes in `ListingScorer.cs`. Change the constants and formulas in that file; tests in `src/backend/tests` should be updated if ranking behavior changes.

- **Numeric type.** Scores use `decimal` for precision. Switch to `float` or `int` (scaled) if this ever becomes a hot path.
- **Over-budget penalty.** Absolute difference treats $1 under and $1 over the same. To prefer cheaper homes, use a signed delta and a harsher curve when price is above target, e.g. `1 / (1 + priceDelta² × 5)` for overages only (the comment notes that 5× is aggressive on large misses).
- **Recency curve.** `1 / (1 + days)` drops quickly in the first weeks. Alternatives already sketched in code: linear (`1 − 0.01 × days`, zero after ~30 days) or exponential (`e^(−0.05 × days)`).
- **Prefer older listings.** Invert recency (or raise days instead of shrinking it) if stale inventory should rank higher (sellers more flexible on price).
- **Weights.** Raising `PriceWeight` (and lowering `RecencyWeight`) makes exact budget matches beat slightly newer near-misses.
