#!/bin/bash

set -e

LETSENCRYPT_ROOT_DIR="/etc/letsencrypt/live";
ROOT_DIR="/var/www/EKTAIOFFICE/Data/certs";

_domains="";

for arg; do
    _domains="$_domains -d $arg";
done

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

mkdir -p ${ROOT_DIR}

certbot certonly --expand --webroot -w ${ROOT_DIR} --noninteractive --agree-tos --email support@$1 $_domains;

cp /etc/letsencrypt/live/$1/fullchain.pem ${ROOT_DIR}/EKTAIOFFICE.crt
cp /etc/letsencrypt/live/$1/privkey.pem ${ROOT_DIR}/EKTAIOFFICE.key
cp /etc/letsencrypt/live/$1/chain.pem ${ROOT_DIR}/stapling.trusted.crt

cat > ${DIR}/letsencrypt_cron.sh <<END
certbot renew >> /var/log/le-renew.log
cp ${LETSENCRYPT_ROOT_DIR}/$1/fullchain.pem ${ROOT_DIR}/EKTAIOFFICE.crt
cp ${LETSENCRYPT_ROOT_DIR}/$1/privkey.pem ${ROOT_DIR}/EKTAIOFFICE.key
cp ${LETSENCRYPT_ROOT_DIR}/$1/chain.pem ${ROOT_DIR}/stapling.trusted.crt
openssl pkcs12 -export -out ${ROOT_DIR}/EKTAIOFFICE.pfx -inkey ${ROOT_DIR}/EKTAIOFFICE.key -in ${ROOT_DIR}/EKTAIOFFICE.crt -password pass:EKTAIOFFICE
chown EKTAIOFFICE:EKTAIOFFICE ${ROOT_DIR}/EKTAIOFFICE.pfx
service nginx reload
END

chmod a+x ${DIR}/letsencrypt_cron.sh

cat > /etc/cron.d/letsencrypt <<END
@weekly root ${DIR}/letsencrypt_cron.sh
END

source $DIR/default-EKTAIOFFICE-ssl.sh
