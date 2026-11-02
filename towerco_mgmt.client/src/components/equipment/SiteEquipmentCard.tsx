import { Card, StateMessage } from '@/components/common';
import { SHARED_OWNER_LABEL } from '@/constants/domain';
import type { EquipmentItem } from '@/types';
import EquipmentTable from './EquipmentTable';

function Group({ title, items }: { title: string; items: EquipmentItem[] }) {
    return (
        <>
            <h3>{title} ({items.length})</h3>
            <StateMessage empty={!items.length} emptyText="None recorded." />
            {!!items.length && <EquipmentTable items={items} />}
        </>
    );
}

function SiteEquipmentCard({ items }: { items: EquipmentItem[] }) {
    const tenantOwned = items.filter(i => i.tenancyId !== null);
    const shared = items.filter(i => i.tenancyId === null);
    return (
        <Card title="Equipment">
            <Group title="Tenant-owned" items={tenantOwned} />
            <Group title={SHARED_OWNER_LABEL} items={shared} />
        </Card>
    );
}

export default SiteEquipmentCard;
