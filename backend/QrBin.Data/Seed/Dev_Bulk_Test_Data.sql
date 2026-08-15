-- Manual dev seed: 100 test bins + parts for the existing Organization, with a realistic
-- status mix (~20% Out of Stock, ~20% Low Stock, ~60% In Stock) — useful for exercising
-- Inventory pagination/search, Dashboard "Needs Attention", and status badges/bars at scale.
-- Run after Initial_Script.sql, Remove_Identity_And_Actor_Tracking.sql, Make_PartNumber_Optional.sql,
-- and Dev_Manager_Seed.sql (needs an Organization to already exist).
-- Skips itself if this batch was already inserted (bin code 'TB-001' already exists).

DO $$
DECLARE
    v_org_id integer;
BEGIN
    IF EXISTS (SELECT 1 FROM "Bin" WHERE "Code" = 'TB-001') THEN
        RAISE NOTICE 'Bulk test data already exists — skipping.';
        RETURN;
    END IF;

    SELECT "Id" INTO v_org_id FROM "Organization" ORDER BY "Id" LIMIT 1;
    IF v_org_id IS NULL THEN
        RAISE EXCEPTION 'No Organization found — run Dev_Manager_Seed.sql first.';
    END IF;

    INSERT INTO "Bin" ("OrganizationId", "Code", "QrToken")
    SELECT v_org_id, 'TB-' || lpad(i::text, 3, '0'), replace(gen_random_uuid()::text, '-', '')
    FROM generate_series(1, 100) AS i;

    INSERT INTO "Part" (
        "OrganizationId", "BinId", "PartName", "PartNumber",
        "CurrentStock", "MinimumStock", "LowStockNotificationSent"
    )
    SELECT
        v_org_id,
        b."Id",
        'Test Part ' || i,
        'TP-' || lpad(i::text, 4, '0'),
        CASE
            WHEN i % 5 = 0 THEN 0                                          -- ~20% Out of Stock
            WHEN i % 5 = 1 THEN GREATEST(1, (5 + (i % 10)) - (i % 3 + 1))  -- ~20% Low Stock
            ELSE (5 + (i % 10)) + 10 + (i % 20)                           -- ~60% In Stock
        END,
        5 + (i % 10),
        false
    FROM generate_series(1, 100) AS i
    JOIN "Bin" b ON b."Code" = 'TB-' || lpad(i::text, 3, '0') AND b."OrganizationId" = v_org_id;

    RAISE NOTICE '100 test bins and parts created.';
END $$;

-- To remove this batch later:
-- DELETE FROM "Part" WHERE "PartNumber" LIKE 'TP-%';
-- DELETE FROM "Bin" WHERE "Code" LIKE 'TB-%';
