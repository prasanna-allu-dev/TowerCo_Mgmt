import { sitesApi } from '@/api';
import { FIELD_ENGINEERS, WORK_ORDER_CATEGORIES, WORK_ORDER_PRIORITIES } from '@/constants/workOrders';
import { useApi } from '@/hooks/useApi';
import type { WorkOrder } from '@/types';
import { useCreateWorkOrderForm } from './useCreateWorkOrderForm';

interface CreateWorkOrderFormProps {
    onCreated: (workOrder: WorkOrder) => void;
    onCancel: () => void;
}

function CreateWorkOrderForm({ onCreated, onCancel }: CreateWorkOrderFormProps) {
    const { data: sites } = useApi(() => sitesApi.list());
    const { draft, update, submit, error, saving } = useCreateWorkOrderForm(onCreated);

    return (
        <form className="wo-form" onSubmit={submit}>
            <label>Site
                <select className="input" required value={draft.siteId} onChange={e => update({ siteId: e.target.value })}>
                    <option value="">Select a site</option>
                    {sites?.map(s => <option key={s.id} value={s.id}>{s.siteCode} · {s.name}</option>)}
                </select>
            </label>
            <label className="wo-span-2">Title
                <input className="input" required value={draft.title} onChange={e => update({ title: e.target.value })} />
            </label>
            <label>Category
                <select className="input" value={draft.category} onChange={e => update({ category: e.target.value as WorkOrder['category'] })}>
                    {WORK_ORDER_CATEGORIES.map(c => <option key={c} value={c}>{c}</option>)}
                </select>
            </label>
            <label>Priority
                <select className="input" value={draft.priority} onChange={e => update({ priority: e.target.value as WorkOrder['priority'] })}>
                    {WORK_ORDER_PRIORITIES.map(p => <option key={p} value={p}>{p}</option>)}
                </select>
            </label>
            <label>Due date
                <input className="input" type="date" required value={draft.dueDate} onChange={e => update({ dueDate: e.target.value })} />
            </label>
            <label>Assignee
                <select className="input" value={draft.assignee} onChange={e => update({ assignee: e.target.value })}>
                    <option value="">Unassigned</option>
                    {FIELD_ENGINEERS.map(n => <option key={n} value={n}>{n}</option>)}
                </select>
            </label>
            <label className="wo-span-3">Description
                <textarea className="input" rows={3} value={draft.description} onChange={e => update({ description: e.target.value })} />
            </label>
            {error && <p className="alert-error wo-span-3">{error}</p>}
            <div className="wo-actions wo-span-3">
                <button type="button" className="wo-btn" onClick={onCancel}>Cancel</button>
                <button type="submit" className="wo-btn primary" disabled={saving}>{saving ? 'Saving...' : 'Create work order'}</button>
            </div>
        </form>
    );
}

export default CreateWorkOrderForm;
