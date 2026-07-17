import { Button } from "@/components/ui/button";
import { logout } from "@/services/AuthService";
import { useNavigate } from "react-router-dom";

export default function LogoutButton() {

    const navigate = useNavigate();

    const handleLogout = () => {

        logout();

        navigate("/HomePage");

    };

    return (

        <Button
            variant="destructive"
            onClick={handleLogout}
        >

            Logout

        </Button>

    );

}