-- Генерация тестовых данных для приложения «Менеджер задач (TODO)».
--
-- Запуск:
--   docker exec -i todo-postgres psql -U todo -d todo_db -f - < scripts/seed-test-data.sql
--   либо просто: powershell -File scripts/seed-test-data.ps1
--
-- Скрипт идемпотентен по справочникам (категории/теги не дублируются),
-- но задачи добавляет при каждом запуске.

\set ON_ERROR_STOP on

DO $$
DECLARE
    v_count      int := 50;   -- сколько задач сгенерировать

    v_work       uuid;
    v_personal   uuid;
    v_study      uuid;

    v_urgent     uuid;
    v_important  uuid;
    v_demo       uuid;
    v_home       uuid;

    v_task       uuid;
    v_prior      text;
    v_status     text;
    v_due        timestamptz;

    v_titles     text[] := ARRAY[
        'Подготовить план спринта',
        'Провести код-ревью',
        'Обновить документацию',
        'Исправить найденные баги',
        'Настроить CI-конвейер',
        'Согласовать требования',
        'Провести планёрку команды',
        'Разобрать почту и задачи',
        'Написать тесты',
        'Обновить зависимости',
        'Разобрать технический долг',
        'Подготовить отчёт'
    ];
    v_priorities text[] := ARRAY['Low', 'Medium', 'High', 'Critical'];
    v_statuses   text[] := ARRAY['New', 'InProgress', 'Done', 'Cancelled'];

    i            int;
BEGIN
    PERFORM setseed(0.4242);  -- воспроизводимость генерации

    -- Справочник категорий
    INSERT INTO categories ("Id", "Name", "Color") VALUES
        (gen_random_uuid(), 'Работа', '#4A90D9'),
        (gen_random_uuid(), 'Личное', '#7ED321'),
        (gen_random_uuid(), 'Учёба',  '#F5A623')
    ON CONFLICT ("Name") DO NOTHING;

    SELECT "Id" INTO v_work     FROM categories WHERE "Name" = 'Работа';
    SELECT "Id" INTO v_personal FROM categories WHERE "Name" = 'Личное';
    SELECT "Id" INTO v_study    FROM categories WHERE "Name" = 'Учёба';

    -- Справочник тегов
    INSERT INTO tags ("Id", "Name") VALUES
        (gen_random_uuid(), 'срочно'),
        (gen_random_uuid(), 'важно'),
        (gen_random_uuid(), 'демо'),
        (gen_random_uuid(), 'дом')
    ON CONFLICT ("Name") DO NOTHING;

    SELECT "Id" INTO v_urgent    FROM tags WHERE "Name" = 'срочно';
    SELECT "Id" INTO v_important FROM tags WHERE "Name" = 'важно';
    SELECT "Id" INTO v_demo      FROM tags WHERE "Name" = 'демо';
    SELECT "Id" INTO v_home      FROM tags WHERE "Name" = 'дом';

    FOR i IN 1..v_count LOOP
        v_prior  := v_priorities[1 + floor(random() * array_length(v_priorities, 1))::int];
        v_status := v_statuses[1 + floor(random() * array_length(v_statuses, 1))::int];
        v_due    := now() + ((random() * 30 - 10) || ' days')::interval;  -- от -10 до +20 дней

        INSERT INTO tasks (
            "Id", "CategoryId", "CompletedAt", "CreatedAt",
            "Description", "DueDate", "Priority", "Status", "Title")
        VALUES (
            gen_random_uuid(),
            CASE 1 + floor(random() * 3)::int
                WHEN 1 THEN v_work
                WHEN 2 THEN v_personal
                ELSE v_study
            END,
            CASE WHEN v_status = 'Done'
                 THEN now() - ((random() * 5) || ' days')::interval
                 ELSE NULL
            END,
            now() - ((random() * 60) || ' days')::interval,
            'Автосгенерированная задача №' || i,
            v_due,
            v_prior,
            v_status,
            v_titles[1 + floor(random() * array_length(v_titles, 1))::int] || ' #' || i
        )
        RETURNING "Id" INTO v_task;

        INSERT INTO task_tags ("TagsId", "TasksId") VALUES (v_demo, v_task)
        ON CONFLICT DO NOTHING;

        IF random() < 0.40 THEN
            INSERT INTO task_tags ("TagsId", "TasksId") VALUES (v_urgent, v_task)
            ON CONFLICT DO NOTHING;
        END IF;

        IF random() < 0.30 THEN
            INSERT INTO task_tags ("TagsId", "TasksId") VALUES (v_important, v_task)
            ON CONFLICT DO NOTHING;
        END IF;

        IF random() < 0.20 THEN
            INSERT INTO task_tags ("TagsId", "TasksId") VALUES (v_home, v_task)
            ON CONFLICT DO NOTHING;
        END IF;
    END LOOP;
END $$;

-- Итоговая сводка
SELECT 'tasks'     AS table_name, COUNT(*) AS rows FROM tasks
UNION ALL SELECT 'categories', COUNT(*) FROM categories
UNION ALL SELECT 'tags',       COUNT(*) FROM tags
UNION ALL SELECT 'task_tags',  COUNT(*) FROM task_tags;
