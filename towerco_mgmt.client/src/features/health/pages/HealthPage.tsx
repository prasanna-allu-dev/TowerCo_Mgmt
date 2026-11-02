import { useState } from 'react';
import { healthApi } from '@/features/health/api';
import { Card, StateMessage } from '@/shared/components';
import { HealthFilters, HealthKpis, HealthTable } from '@/features/health/components';
import { useApi } from '@/shared/hooks/useApi';
import PageHeader from '@/shared/components/PageHeader';
import type { HealthFilter } from '@/features/health/types';

function HealthPage() {
    const [filter, setFilter] = useState<HealthFilter>({});
    const { data, error, loading } = useApi(() => healthApi.portfolio(filter), [filter.country, filter.status]);

    return (
        <>
            <PageHeader title="Tower health" subtitle="Telemetry, inspections and open work orders per site. Worst signal sets the health." />
            {data && <HealthKpis health={data} />}
            <Card>
                <HealthFilters value={filter} onChange={setFilter} />
                <StateMessage loading={loading} error={error} empty={data?.sites.length === 0} emptyText="No sites match these filters." />
                {!!data?.sites.length && <HealthTable sites={data.sites} />}
            </Card>
        </>
    );
}

export default HealthPage;
