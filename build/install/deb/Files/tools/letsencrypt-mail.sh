#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
ROOT_DIR="/var/www/EKTAIOFFICE/Data/certs";
LETSENCRYPT_ROOT_DIR="/etc/letsencrypt/live";

certbot certonly --expand --webroot -w ${ROOT_DIR} --noninteractive --agree-tos --email support@$1 $1;

cp ${LETSENCRYPT_ROOT_DIR}/$1/fullchain.pem ${ROOT_DIR}/mail.EKTAIOFFICE.crt
cp ${LETSENCRYPT_ROOT_DIR}/$1/privkey.pem ${ROOT_DIR}/mail.EKTAIOFFICE.key

cat > ${DIR}/letsencrypt_mail_cron.sh <<END
certbot renew >> /var/log/le-renew.log
cp ${LETSENCRYPT_ROOT_DIR}/$1/fullchain.pem ${ROOT_DIR}/mail.EKTAIOFFICE.crt
cp ${LETSENCRYPT_ROOT_DIR}/$1/privkey.pem ${ROOT_DIR}/mail.EKTAIOFFICE.key
END

chmod a+x ${DIR}/letsencrypt_mail_cron.sh

cat > /etc/cron.d/letsencrypt_mail <<END
@weekly root ${DIR}/letsencrypt_mail_cron.sh
END
