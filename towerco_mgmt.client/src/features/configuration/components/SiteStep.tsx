import { COUNTRIES, SITE_CODE_EXAMPLE } from '@/shared/constants/domain';
import FormField from './FormField';
import { bind } from './stepProps';
import type { StepProps } from './stepProps';

function SiteStep(props: StepProps) {
    const { errors } = props;
    return (
        <div className="form-grid">
            <FormField label="Site code" error={errors.siteCode} hint={`Country-City-Number, e.g. ${SITE_CODE_EXAMPLE}`}>
                <input {...bind(props, 'siteCode')} placeholder={SITE_CODE_EXAMPLE} />
            </FormField>
            <FormField label="Site name" error={errors.name}>
                <input {...bind(props, 'name')} placeholder="Hinjewadi Phase 2" />
            </FormField>
            <FormField label="Country" error={errors.countryCode}>
                <select {...bind(props, 'countryCode')}>
                    <option value="">Select country</option>
                    {COUNTRIES.map(c => <option key={c.code} value={c.code}>{c.name}</option>)}
                </select>
            </FormField>
            <FormField label="City" error={errors.city}>
                <input {...bind(props, 'city')} placeholder="Pune" />
            </FormField>
            <FormField label="Latitude" error={errors.latitude}>
                <input {...bind(props, 'latitude')} type="number" step="any" placeholder="18.5913" />
            </FormField>
            <FormField label="Longitude" error={errors.longitude}>
                <input {...bind(props, 'longitude')} type="number" step="any" placeholder="73.7389" />
            </FormField>
        </div>
    );
}

export default SiteStep;
