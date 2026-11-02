import { Fragment } from 'react';
import { COUNTRIES, DEFAULT_NEW_SITE_STATUS } from '@/constants/domain';
import type { FormValues } from './createSiteForm';

function countryName(code: string): string {
    return COUNTRIES.find(c => c.code === code)?.name ?? code;
}

function ReviewStep({ values }: { values: FormValues }) {
    const rows: [string, string][] = [
        ['Site code', values.siteCode.toUpperCase()],
        ['Name', values.name],
        ['Location', `${values.city}, ${countryName(values.countryCode)}`],
        ['Coordinates', `${values.latitude}, ${values.longitude}`],
        ['Tower', `${values.towerType}, ${values.heightMetres} m`],
        ['Tenant slots', `${values.maxTenants} (0 used)`],
        ['Initial status', DEFAULT_NEW_SITE_STATUS],
    ];
    return (
        <dl className="details">
            {rows.map(([label, value]) => <Fragment key={label}><dt>{label}</dt><dd>{value}</dd></Fragment>)}
        </dl>
    );
}

export default ReviewStep;
