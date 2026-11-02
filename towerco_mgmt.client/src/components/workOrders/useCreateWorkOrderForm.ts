import { useState } from 'react';
import type { FormEvent } from 'react';
import { workOrdersApi } from '@/api';
import { API_ERROR_FALLBACK } from '@/constants/api';
import { MS_PER_DAY } from '@/constants/domain';
import { DEFAULT_DUE_IN_DAYS } from '@/constants/workOrders';
import type { CreateWorkOrderRequest, WorkOrder } from '@/types';

export type WorkOrderDraft = Omit<CreateWorkOrderRequest, 'siteId' | 'assignee'> & { siteId: string; assignee: string };

const defaultDueDate = () => new Date(Date.now() + DEFAULT_DUE_IN_DAYS * MS_PER_DAY).toISOString().slice(0, 10);

const emptyDraft = (): WorkOrderDraft => ({
    siteId: '', title: '', description: '', category: 'Corrective', priority: 'Medium', dueDate: defaultDueDate(), assignee: '',
});

const toRequest = (draft: WorkOrderDraft): CreateWorkOrderRequest =>
    ({ ...draft, siteId: Number(draft.siteId), assignee: draft.assignee || null });

export function useCreateWorkOrderForm(onCreated: (workOrder: WorkOrder) => void) {
    const [draft, setDraft] = useState<WorkOrderDraft>(emptyDraft);
    const [error, setError] = useState<string>();
    const [saving, setSaving] = useState(false);

    const update = (patch: Partial<WorkOrderDraft>) => setDraft(current => ({ ...current, ...patch }));

    const submit = (e: FormEvent) => {
        e.preventDefault();
        setSaving(true);
        setError(undefined);
        workOrdersApi.create(toRequest(draft))
            .then(created => { setDraft(emptyDraft()); onCreated(created); })
            .catch((err: unknown) => setError(err instanceof Error ? err.message : API_ERROR_FALLBACK))
            .finally(() => setSaving(false));
    };

    return { draft, update, submit, error, saving };
}
