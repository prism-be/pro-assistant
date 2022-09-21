import {NextPage} from "next";
import useTranslation from 'next-translate/useTranslation'

const Login: NextPage = () => {

    const { t, lang } = useTranslation('common');
    
    return <div>
        {t("title")}
    </div>
}

export default Login;