aria2 1.37.0, static build from https://github.com/abcfy2/aria2-static-build/releases/tag/1.37.0
(`aria2-aarch64-linux-musl_static.zip`). aria2 is distributed under the GNU GPL version 2 or later
(COPYING), with an exception for linking with OpenSSL; its source and that of each dependency is linked below.

## Build Info - aarch64-unknown-linux-musl With OpenSSL and zlib-ng
Building using these dependencies:
- zlib-ng: Version: 1.3.1.zlib-ng, source: https://github.com/zlib-ng/zlib-ng/archive/refs/tags/2.3.3.tar.gz
- xz: Version: 5.8.3, source: https://sourceforge.net/projects/lzmautils/files/xz-5.8.3.tar.xz
- openssl: Version: 3.6.2, source: https://github.com/openssl/openssl/releases/download/openssl-3.6.2/openssl-3.6.2.tar.gz
- libiconv: 1.19, source: https://ftpmirror.gnu.org/libiconv/libiconv-1.19.tar.gz
- libxml2: Version: 2.15.1, source: https://download.gnome.org/sources/libxml2/2.15/libxml2-2.15.1.tar.xz
- sqlite: Version: 3.53.1, source: https://github.com/sqlite/sqlite/archive/refs/tags/version-3.53.1.tar.gz
- c-ares: Version: 1.34.5, source: https://github.com/c-ares/c-ares/releases/download/v1.34.5/c-ares-1.34.5.tar.gz
- libssh2: Version: 1.11.1, source: https://libssh2.org/download/libssh2-1.11.1.tar.xz
- aria2: source: https://github.com/aria2/aria2/releases/download/release-1.37.0/aria2-1.37.0.tar.gz

aria2 version info:
```txt
aria2 version 1.37.0
Copyright (C) 2006, 2019 Tatsuhiro Tsujikawa

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

** Configuration **
Enabled Features: Async DNS, BitTorrent, Firefox3 Cookie, GZip, HTTPS, Message Digest, Metalink, XML-RPC, SFTP
Hash Algorithms: sha-1, sha-224, sha-256, sha-384, sha-512, md5, adler32
Libraries: zlib/1.3.1.zlib-ng libxml2/2.15.1 sqlite3/3.53.1 OpenSSL/3.6.0b c-ares/1.34.5 libssh2/1.11.1
Compiler: clang 22.1.4 (https://github.com/llvm/llvm-project 35990504507d79e0b9deb809c8ee5e1b34ceef20)
  built by  x86_64-pc-linux-musl
  targeting aarch64-unknown-linux-musl
  on        May 10 2026 14:08:19
System: Linux 6.17.0-1010-azure #10~24.04.1-Ubuntu SMP Fri Mar  6 22:00:57 UTC 2026 aarch64

Report bugs to https://github.com/aria2/aria2/issues
Visit https://aria2.github.io/
```
