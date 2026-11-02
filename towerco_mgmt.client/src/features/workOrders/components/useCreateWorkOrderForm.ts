import { useState } from 'react';
import type { FormEvent } from 'react';
import { workOrdersApi } from '@/features/workOrders/api';
import { MS_PER_DAY } from '@/shared/constants/domain';
import { DEFAULT_DUE_IN_DAYS } from '@/features/workOrders/constants/workOrders';
import type { CreateWorkOrderRequest, WorkOrder } from '@/features/workOrders/types';
import { useAsyncAction } from '@/shared/hooks/useAsyncAction';

export type WorkOrderDraft = Omit<CreateWorkOrderRequest, 'siteId' | 'assignee'> & { siteId: string; assignee: string };

const defaultDueDate = () => new Date(Date.now() + DEFAULT_DUE_IN_DAYS * MS_PER_DAY).toISOString().slice(0, 10);

const emptyDraft = (): WorkOrderDraft => ({
    siteId: '', title: '', description: '', category: 'Corrective', priority: 'Medium', dueDate: defaultDueDate(), assignee: '',
});

const toRequest = (draft: WorkOrderDraft): CreateWorkOrderRequest =>
    ({ ...draft, siteId: Number(draft.siteId), assignee: draft.assignee || null });

export function useCreateWorkOrderForm(onCreated: (workOrder: WorkOrder) => void) {
    const [draft, setDraft] = useState<WorkOrderDraft>(emptyDraft);
    const save = useAsyncAction(workOrdersApi.create, (created: WorkOrder) => { setDraft(emptyDraft()); onCreated(created); });

    const update = (patch: Partial<WorkOrderDraft>) => setDraft(current => ({ ...current, ...patch }));
    const submit = (e: FormEvent) => {
        e.preventDefault();
        void save.run(toRequest(draft));
    };

    return { draft, update, submit, error: save.error, saving: save.pending };
}
