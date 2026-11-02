import { useState } from 'react';
import { sitesApi } from '@/features/sites/api';
import { Card, StateMessage } from '@/shared/components';
import { SiteFilters, SitesTable } from '@/features/sites/components';
import { useApi } from '@/shared/hooks/useApi';
import PageHeader from '@/shared/components/PageHeader';
import { countLabel } from '@/shared/utils/format';
import type { SiteFilter } from '@/features/sites/types';

function SitesPage() {
    const [filter, setFilter] = useState<SiteFilter>({});
    const { data, error, loading } = useApi(() => sitesApi.list(filter), [filter.country, filter.status, filter.search]);
    const hasRows = !loading && !error && !!data?.length;

    return (
        <>
            <PageHeader title="Sites" subtitle="Search and filter tower sites across the portfolio" />
            <Card title="Filters"><SiteFilters value={filter} onChange={setFilter} /></Card>
            <Card title="Results" subtitle={countLabel(data?.length, 'site', 'sites')}>
                <StateMessage loading={loading} error={error} empty={data?.length === 0} emptyText="No sites match the filters." />
                {hasRows && <SitesTable sites={data} />}
            </Card>
        </>
    );
}

export default SitesPage;
