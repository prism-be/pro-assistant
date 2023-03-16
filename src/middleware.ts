import { withMiddlewareAuthRequired } from "@auth0/nextjs-auth0/edge";
import { withHeaders } from "@/modules/middlewares/headers";

export default withHeaders(withMiddlewareAuthRequired());
