import { Navigate } from 'react-router-dom';
import { getPersona } from '@/app/personas';
import { usePersona } from '@/app/usePersona';

function LandingRedirect() {
    const { persona } = usePersona();
    return <Navigate to={getPersona(persona).landingRoute} replace />;
}

export default LandingRedirect;
