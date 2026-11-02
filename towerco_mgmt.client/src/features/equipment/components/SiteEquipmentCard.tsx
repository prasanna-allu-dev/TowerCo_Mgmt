import { useMemo } from 'react';
import { Card, StateMessage } from '@/shared/components';
import type { EquipmentItem } from '@/features/equipment/types';
import EquipmentTable from './EquipmentTable';

function Group({ title, items }: { title: string; items: EquipmentItem[] }) {
    return (
        <>
            <h3>{title} ({items.length})</h3>
            <EquipmentTable items={items} />
        </>
    );
}

/** Every item belongs to a tenancy, so the site's equipment is grouped by owning operator. */
function groupByOwner(items: EquipmentItem[]): [string, EquipmentItem[]][] {
    const groups = new Map<string, EquipmentItem[]>();
    items.forEach(item => groups.set(item.ownerName, [...(groups.get(item.ownerName) ?? []), item]));
    return [...groups.entries()];
}

function SiteEquipmentCard({ items }: { items: EquipmentItem[] }) {
    const byOwner = useMemo(() => groupByOwner(items), [items]);
    return (
        <Card title="Equipment">
            <StateMessage empty={!items.length} emptyText="No equipment recorded." />
            {byOwner.map(([owner, ownerItems]) => <Group key={owner} title={owner} items={ownerItems} />)}
        </Card>
    );
}

export default SiteEquipmentCard;
