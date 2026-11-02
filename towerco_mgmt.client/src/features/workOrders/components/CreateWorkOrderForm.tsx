import { sitesApi } from '@/features/sites/api';
import type { SiteSummary } from '@/features/sites/types';
import { FIELD_ENGINEERS, WORK_ORDER_CATEGORIES, WORK_ORDER_PRIORITIES } from '@/features/workOrders/constants/workOrders';
import type { WorkOrder } from '@/features/workOrders/types';
import { SelectField } from '@/shared/components';
import { useApi } from '@/shared/hooks/useApi';
import { useCreateWorkOrderForm } from './useCreateWorkOrderForm';

type FormState = ReturnType<typeof useCreateWorkOrderForm>;

interface CreateWorkOrderFormProps {
    onCreated: (workOrder: WorkOrder) => void;
    onCancel: () => void;
}

const toSiteOption = (s: SiteSummary) => ({ value: String(s.id), label: `${s.siteCode} · ${s.name}` });

function ClassificationFields({ draft, update }: Pick<FormState, 'draft' | 'update'>) {
    return (
        <>
            <SelectField label="Category" value={draft.category} options={WORK_ORDER_CATEGORIES}
                onChange={v => update({ category: v as WorkOrder['category'] })} />
            <SelectField label="Priority" value={draft.priority} options={WORK_ORDER_PRIORITIES}
                onChange={v => update({ priority: v as WorkOrder['priority'] })} />
            <label>Due date
                <input className="input" type="date" required value={draft.dueDate} onChange={e => update({ dueDate: e.target.value })} />
            </label>
            <SelectField label="Assignee" value={draft.assignee} options={FIELD_ENGINEERS} placeholder="Unassigned"
                onChange={v => update({ assignee: v })} />
        </>
    );
}

function FormActions({ saving, onCancel }: { saving: boolean; onCancel: () => void }) {
    return (
        <div className="wo-actions wo-span-3">
            <button type="button" className="wo-btn" onClick={onCancel}>Cancel</button>
            <button type="submit" className="wo-btn primary" disabled={saving}>{saving ? 'Saving...' : 'Create work order'}</button>
        </div>
    );
}

function CreateWorkOrderForm({ onCreated, onCancel }: CreateWorkOrderFormProps) {
    const { data: sites } = useApi(() => sitesApi.list());
    const { draft, update, submit, error, saving } = useCreateWorkOrderForm(onCreated);

    return (
        <form className="wo-form" onSubmit={submit}>
            <SelectField label="Site" required value={draft.siteId} options={sites?.map(toSiteOption) ?? []}
                placeholder="Select a site" onChange={v => update({ siteId: v })} />
            <label className="wo-span-2">Title
                <input className="input" required value={draft.title} onChange={e => update({ title: e.target.value })} />
            </label>
            <ClassificationFields draft={draft} update={update} />
            <label className="wo-span-3">Description
                <textarea className="input" rows={3} value={draft.description} onChange={e => update({ description: e.target.value })} />
            </label>
            {error && <p className="alert-error wo-span-3">{error}</p>}
            <FormActions saving={saving} onCancel={onCancel} />
        </form>
    );
}

export default CreateWorkOrderForm;
